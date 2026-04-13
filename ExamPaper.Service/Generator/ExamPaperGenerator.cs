using System;
using System.Collections.Generic;
using System.Linq;

namespace ExamPaper.Service.Generator;


public class ExamPaperGenerator
{
    private readonly List<string> _questions;

    private readonly int _ticketsCount;
    private readonly int _questionsPerTicket;

    public ExamPaperGenerator(List<string> questions, int ticketsCount, int questionsPerTicket)
    {
        _questions = questions;
        _ticketsCount = ticketsCount;
        _questionsPerTicket = questionsPerTicket;
    }

    public List<IExamPaper> GenerateTickets()
    {
        var tickets = new List<IExamPaper>();
        var random = new Random();
        
        var shuffledQuestions = _questions.OrderBy(x => random.Next()).ToList();
        
        int index = 0;
        
        for (int ticketNum = 1; ticketNum <= _ticketsCount; ticketNum++)
        {
            var ticketQuestions = new List<string>();
            
            for (int i = 0; i < _questionsPerTicket; i++)
            {
                if (index >= shuffledQuestions.Count)
                    index = 0;
                    
                ticketQuestions.Add(shuffledQuestions[index]);
                index++;
            }
            
            tickets.Add(new IExamPaper 
            { 
                Number = ticketNum, 
                Questions = ticketQuestions 
            });
        }
        
        return tickets;
    }
}