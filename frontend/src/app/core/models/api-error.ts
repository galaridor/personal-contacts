export interface ApiError {
  message: string;
  fieldErrors: Record<string, string[]>;
}

export interface ProblemDetails {
  title?: string;
  detail?: string;
  status?: number;
  errors?: Record<string, string[]>;
}
