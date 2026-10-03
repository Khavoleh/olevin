/** The API answered with a non-2xx status. */
export class ApiError extends Error {
  constructor(readonly status: number) {
    super(`The API responded with status ${status}.`);
    this.name = "ApiError";
  }
}
