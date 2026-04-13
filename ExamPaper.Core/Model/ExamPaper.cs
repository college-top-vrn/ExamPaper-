using System.Collections.ObjectModel;

using ExamPaper.Core.Interfaces;

namespace ExamPaper.Core.Model;



    public sealed class ExamPaper : IExamPaper
    {
        private readonly List<IQuestion> _questions = new();

        public string Title { get; private set; }

        public IReadOnlyCollection<IQuestion> Questions => new ReadOnlyCollection<IQuestion>(_questions);

        public ExamPaper(string title)
        {
            Title = title;
            Validate();
        }

        public void AddQuestion(IQuestion question)
        {
            if (question is null)
                throw new ArgumentNullException(nameof(question));

            question.Validate();
            _questions.Add(question);
        }

        public bool RemoveQuestion(Guid questionId)
        {
            var question = _questions.FirstOrDefault(q => q.Id == questionId);
            if (question is null)
                return false;

            return _questions.Remove(question);
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Title))
                throw new ArgumentException("Title must be filled.", nameof(Title));
        }
    }
