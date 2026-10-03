import {
	LANGUAGE_COOKIE,
	PAGE_URLS,
	PUBLIC_PAGE_URLS,
} from "@shared/constants";
import {
	getIsAuthenticated,
	getLanguage,
	getLanguageUrl,
	getPathLanguage,
} from "@shared/helpers";
import { type NextRequest, NextResponse } from "next/server";

/** Redirects to another path of the same origin, keeping the query string. */
function redirectTo(request: NextRequest, pathname: string): NextResponse {
	const url = request.nextUrl.clone();
	url.pathname = pathname;

	// The target depends on a cookie and headers that can change, so it must not be cached.
	return NextResponse.redirect(url, {
		headers: { "Cache-Control": "no-store" },
	});
}

/**
 * Puts the language into the URL and lets only signed-in users into non-public pages.
 * Both checks run here, so the pages themselves stay static.
 */
export async function proxy(request: NextRequest) {
	const { pathname } = request.nextUrl;
	const language = getPathLanguage(pathname);

	if (!language) {
		const preferred = getLanguage(
			request.cookies.get(LANGUAGE_COOKIE.NAME)?.value,
			request.headers.get("accept-language"),
		);

		return redirectTo(request, getLanguageUrl(pathname, preferred));
	}

	const page = pathname.slice(language.length + 1) || PAGE_URLS.HOME;

	if (PUBLIC_PAGE_URLS.includes(page) || (await getIsAuthenticated(request))) {
		return NextResponse.next();
	}

	return redirectTo(request, getLanguageUrl(PAGE_URLS.SIGN_IN, language));
}

/** Requests the proxy runs for: everything except the API, Next.js internals, auth callback, health check and files. */
export const config = {
	matcher: ["/((?!api|_next|callback|health|.*\\..*).*)"],
};
