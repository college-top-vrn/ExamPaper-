using System;
using System.Collections.Generic;
using System.Linq;
using ExamPaper.Core.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ExamPaper.Infrastructure.Exporter;

/// <summary>
/// Класс-стратегия для экспорта экзаменационных билетов в PDF.
/// </summary>
public sealed class PdfExamExporter : IExamExporter
{
    /// <summary>
    /// Метод для экспорта списка билетов в PDF, закодированный в байтах,
    /// с использованием QuestPDF.
    /// </summary>
    /// <param name="examPapers">Список билетов.</param>
    /// <returns>Массив байтов PDF.</returns>
    /// <exception cref="ArgumentException">Если список билетов пуст.</exception>
    public byte[] Export(IEnumerable<IExamPaper> examPapers)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var papers = examPapers.ToList();

        if (papers.Count == 0)
        {
            throw new ArgumentException("No exam papers have been provided.");
        }

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                // Настройки страницы
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                // Верхний колонтитул страницы
                page.Header().Text("Экзаменационные билеты").SemiBold().FontSize(20);

                // Основной контент страницы
                page.Content()
                    .Column(column =>
                    {
                        foreach (var paper in papers)
                        {
                            column
                                .Item()
                                .PaddingVertical(10)
                                .Column(paperColumn =>
                                {
                                    // Заголовок билета
                                    paperColumn
                                        .Item()
                                        .Row(row =>
                                        {
                                            row.RelativeItem()
                                                .Text($"Билет: {paper.Title}")
                                                .Bold()
                                                .FontSize(16);
                                            row.ConstantItem(100)
                                                .Text($"ID: {paper.Id.ToString()}")
                                                .FontSize(9);
                                        });

                                    // Вопросы
                                    paperColumn
                                        .Item()
                                        .PaddingLeft(15)
                                        .Column(questionsColumn =>
                                        {
                                            int index = 1;
                                            foreach (var question in paper.Questions)
                                            {
                                                questionsColumn
                                                    .Item()
                                                    .PaddingTop(5)
                                                    .Text(t =>
                                                    {
                                                        t.Span($"{index}. ").Bold();
                                                        t.Span(question.Text);
                                                    });
                                                index++;
                                            }
                                        });
                                });

                            // Разделитель между билетами
                            if (paper != papers.Last())
                                column
                                    .Item()
                                    .PaddingVertical(5)
                                    .LineHorizontal(1)
                                    .LineColor(Colors.Grey.Lighten2);
                        }
                    });

                // Нижний колонтитул страницы
                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Страница ");
                        x.CurrentPageNumber();
                        x.Span(" из ");
                        x.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();
    }
}
