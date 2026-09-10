export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
}

export interface ContactsPageRequest {
  search?: string;
  page: number;
  pageSize: number;
}

export const DEFAULT_PAGE_SIZE = 10;

export function lastPage(totalCount: number, pageSize: number): number {
  return Math.max(1, Math.ceil(Math.max(0, totalCount) / pageSize));
}
