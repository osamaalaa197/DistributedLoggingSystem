export interface ResponsAPI<T = any> {
  isSuccess: boolean;
  message: string;
  result: T;
  totalRecords: number;
  PageNumber: number | null;
  PageSize: number | null;
}
