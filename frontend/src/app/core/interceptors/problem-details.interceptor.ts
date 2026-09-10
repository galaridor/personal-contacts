import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

import { ApiError, ProblemDetails } from '../models';

export const problemDetailsInterceptor: HttpInterceptorFn = (request, next) =>
  next(request).pipe(catchError((error: HttpErrorResponse) => throwError(() => toApiError(error))));

function toApiError(error: HttpErrorResponse): ApiError {
  if (error.status === 0) {
    return {
      message: 'The server could not be reached. Check your connection and try again.',
      fieldErrors: {},
    };
  }

  const problem = (error.error ?? {}) as ProblemDetails;
  const fieldErrors = problem.errors ?? {};

  return {
    message: messageFor(error.status, problem, fieldErrors),
    fieldErrors,
  };
}

function messageFor(
  status: number,
  problem: ProblemDetails,
  fieldErrors: Record<string, string[]>,
): string {
  if (Object.keys(fieldErrors).length > 0) {
    return 'Please correct the highlighted fields and try again.';
  }

  const message = problem.detail ?? problem.title;
  if (message) {
    return message;
  }

  return status === 404
    ? 'That contact no longer exists.'
    : 'Something went wrong. Please try again.';
}
