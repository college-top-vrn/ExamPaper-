export interface QuestionDto {
  id: string;        // UUID
  text: string;
  createdAt: string; // ISO date
}

export interface CreateQuestionDto {
  text: string;
}
