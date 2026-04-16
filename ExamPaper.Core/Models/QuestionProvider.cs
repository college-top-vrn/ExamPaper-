using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

using ExamPaper.Core.Interfaces;

namespace ExamPaper.Core.Models;

/// <summary>
/// Реализация провайдера вопросов для загрузки данных из файла
/// </summary>
public class QuestionProvider : IQuestionProvider
{
    private readonly string _filePath;

    /// <summary>
    /// Инициализирует новый экземпляр провайдера вопросов
    /// </summary>
    /// <param name="filePath">Путь к файлу с вопросами</param>
    /// <exception cref="ArgumentException">Выбрасывается, если путь к файлу пуст или содержит только пробелы.</exception>
    /// <exception cref="FileNotFoundException">Выбрасывается, если файл по указанному пути не найден.</exception>
    public QuestionProvider(string filePath)
    {
        _filePath = filePath;
    }

    /// <summary>
    /// Читает содержимое файла
    /// </summary>
    /// <returns>Содержимое файла в виде строки. Если файл не найден, возвращает пустую строку</returns>
    private string ReadFile()
    {
        if (!File.Exists(_filePath))
        {
            return string.Empty;
        }
        var text = File.ReadAllText(_filePath);
        return text;
    }

    /// <summary>
    /// Десериализует JSON-строку в коллекцию вопросов
    /// </summary>
    /// <returns>Коллекция вопросов, реализующих <see cref="IQuestion"/>. При ошибке десериализации возвращает пустую коллекцию</returns>
    private IEnumerable<IQuestion> JsonDeserialize()
    {
        var text = ReadFile();
        return JsonSerializer.Deserialize<IEnumerable<IQuestion>>(text) ?? [];
    }

    /// <summary>
    /// Получает все вопросы из JSON-файла
    /// </summary>
    /// <returns>Перечисление всех вопросов из файла</returns>
    public IEnumerable<IQuestion> GetAllQuestions()
    {
        return JsonDeserialize();
    }
}