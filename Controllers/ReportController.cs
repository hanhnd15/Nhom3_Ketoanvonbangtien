using Microsoft.AspNetCore.Mvc;
using MoneyManagementApp.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.IO;

namespace MoneyManagementApp.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult ExportToExcel(string startDate = "", string endDate = "")
        {
            // Lấy dữ liệu thu nhập và chi phí
            var income = _context.Transactions.Where(t => t.TransactionType == "Income").ToList();
            var expense = _context.Transactions.Where(t => t.TransactionType == "Expense").ToList();

            // Lọc theo ngày tháng nếu có
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                DateTime start = DateTime.Parse(startDate);
                DateTime end = DateTime.Parse(endDate);
                income = income.Where(t => t.TransactionDate >= start && t.TransactionDate <= end).ToList();
                expense = expense.Where(t => t.TransactionDate >= start && t.TransactionDate <= end).ToList();
            }

            // Nhóm thu nhập và chi phí theo ngày
            var incomeGrouped = GroupTransactions(income);
            var expenseGrouped = GroupTransactions(expense);

            // Tạo file Excel với EPPlus
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Report");

                // Đặt tiêu đề cho các cột
                worksheet.Cells[1, 1].Value = "Ngày";
                worksheet.Cells[1, 2].Value = "Thu Nhập";
                worksheet.Cells[1, 3].Value = "Chi Phí";
                worksheet.Cells[1, 4].Value = "Chênh Lệch";

                // Điền dữ liệu vào Excel
                int row = 2; // Dòng bắt đầu điền dữ liệu
                var allLabels = incomeGrouped.Select(i => i.Key).Union(expenseGrouped.Select(e => e.Key)).ToList();
                foreach (var label in allLabels)
                {
                    decimal incomeAmount = incomeGrouped.FirstOrDefault(i => i.Key == label).Value;
                    decimal expenseAmount = expenseGrouped.FirstOrDefault(e => e.Key == label).Value;

                    worksheet.Cells[row, 1].Value = label;
                    worksheet.Cells[row, 2].Value = incomeAmount;
                    worksheet.Cells[row, 3].Value = expenseAmount;
                    worksheet.Cells[row, 4].Value = incomeAmount - expenseAmount;

                    row++;
                }

                // Định dạng Excel (optional)
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Tạo tên file theo định dạng "Báo cáo thu chi từ ngày nào đến ngày nào"
                string fileName = $"Báo cáo thu chi từ {startDate} đến {endDate}.xlsx";

                // Lưu file vào bộ nhớ
                var fileContent = package.GetAsByteArray();

                // Trả về file Excel cho người dùng
                return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }


        public IActionResult Index(string startDate = "", string endDate = "")
        {
            // Lấy dữ liệu thu nhập và chi phí
            var income = _context.Transactions.Where(t => t.TransactionType == "Income").ToList();
            var expense = _context.Transactions.Where(t => t.TransactionType == "Expense").ToList();

            // Lọc theo ngày tháng nếu có (từ startDate đến endDate)
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                DateTime start = DateTime.Parse(startDate);
                DateTime end = DateTime.Parse(endDate);
                income = income.Where(t => t.TransactionDate >= start && t.TransactionDate <= end).ToList();
                expense = expense.Where(t => t.TransactionDate >= start && t.TransactionDate <= end).ToList();
            }

            // Nhóm thu nhập và chi phí theo ngày
            var incomeGrouped = GroupTransactions(income);
            var expenseGrouped = GroupTransactions(expense);

            // Tính tổng thu nhập và chi phí
            decimal totalIncome = incomeGrouped.Sum(g => g.Value);
            decimal totalExpense = expenseGrouped.Sum(g => g.Value);

            // Truyền dữ liệu vào View
            var reportModel = new
            {
                IncomeGrouped = incomeGrouped,
                ExpenseGrouped = expenseGrouped,
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                StartDate = startDate,
                EndDate = endDate
            };

            return View(reportModel);
        }

        private List<KeyValuePair<string, decimal>> GroupTransactions(List<Transaction> transactions)
        {
            var grouped = new List<KeyValuePair<string, decimal>>();

            foreach (var item in transactions)
            {
                string key = item.TransactionDate.ToString("yyyy-MM-dd");
                bool found = false;

                // Thay thế lambda bằng vòng lặp for để tìm kiếm nhóm có thời gian trùng khớp
                for (int i = 0; i < grouped.Count; i++)
                {
                    if (grouped[i].Key == key)
                    {
                        grouped[i] = new KeyValuePair<string, decimal>(grouped[i].Key, grouped[i].Value + item.Amount);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    grouped.Add(new KeyValuePair<string, decimal>(key, item.Amount));
                }
            }

            return grouped;
        }
    }
}
