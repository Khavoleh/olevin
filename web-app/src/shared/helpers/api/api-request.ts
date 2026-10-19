import { ApiError } from "./api-error/api-error";
import { getApiToken } from "./get-api-token";
import { getApiUrl } from "./get-api-url";

/**
 * Transport of the Orval clients in `features/<tag>/api`: resolves the path against the API address and adds the
 * user's access token. Runs only on the server.
 */
export async function apiRequest<T>(
	path: string,
	options: RequestInit,
): Promise<T> {
	const token = await getApiToken();

	if (!token) {
		throw new ApiError({ status: 401 });
	}

	const headers = new Headers(options.headers);
	headers.set("Authorization", `Bearer ${token}`);

	const response = await fetch(`${getApiUrl()}${path}`, {
		...options,
		headers,
	});
	const body = await response.text();

	return {
		data: body ? JSON.parse(body) : undefined,
		status: response.status,
		headers: response.headers,
	} as T;
}
