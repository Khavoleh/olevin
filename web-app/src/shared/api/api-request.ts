import { TRACEPARENT_HEADER } from "@shared/constants";
import {
	ApiError,
	createTraceparent,
	getApiToken,
	getApiUrl,
} from "@shared/helpers";
import { headers as getRequestHeaders } from "next/headers";

/**
 * Transport of the Orval clients in `features/<tag>/api`: resolves the path against the API address and adds the
 * user's access token and the trace of the incoming request, so calls of one Server Action share a trace. The data is
 * the parsed JSON, a `Blob` for a downloaded file, or `undefined` without a body. Runs only on the server.
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
	headers.set(
		TRACEPARENT_HEADER,
		(await getRequestHeaders()).get(TRACEPARENT_HEADER) ?? createTraceparent(),
	);

	const response = await fetch(`${getApiUrl()}${path}`, {
		...options,
		headers,
	});
	const isFile = (response.headers.get("content-disposition") ?? "").startsWith(
		"attachment",
	);
	const getData = async () => {
		if (isFile) return response.blob();

		const body = await response.text();

		return body ? JSON.parse(body) : undefined;
	};

	return {
		data: await getData(),
		status: response.status,
		headers: response.headers,
	} as T;
}
