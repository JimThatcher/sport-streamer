import { AxiosError } from "axios";

export const extractErrorMessage = (error: AxiosError, defaultMessage: string) => (
  defaultMessage
);
// (error.response && error.response.data ? error.response.data : error.message) || defaultMessage
