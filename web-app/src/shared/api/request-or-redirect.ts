import { PAGE_URLS } from "@shared/constants";
import { ApiError, getLanguageUrl } from "@shared/helpers";
import { redirect } from "next/navigation";

/** Answer of the API; without a session opens sign-in. */
export async function requestOrRedirect<T>(
	request: () => Promise<T>,
	language: string,
): Promise<T> {
	return request().catch((error) => {
		if (error instanceof ApiError && error.status === 401) {
			console.info("API answered 401, opening sign-in");

			redirect(getLanguageUrl(PAGE_URLS.SIGN_IN, language));
		}

		throw error;
	});
}
