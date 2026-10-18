import {
	LANGUAGE_COOKIE,
	PAGE_URLS,
	PUBLIC_PAGE_URLS,
} from "@shared/constants";
import {
	getIsAuthenticated,
	getIsOnboarded,
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

/** Adds the language to the URL and lets only signed-in, onboarded users into the app. */
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

	if (PUBLIC_PAGE_URLS.includes(page)) {
		return NextResponse.next();
	}

	if (!(await getIsAuthenticated(request))) {
		return redirectTo(request, getLanguageUrl(PAGE_URLS.SIGN_IN, language));
	}

	const isOnboarding =
		page === PAGE_URLS.ONBOARDING ||
		page.startsWith(`${PAGE_URLS.ONBOARDING}/`);

	if (isOnboarding || getIsOnboarded(request)) {
		return NextResponse.next();
	}

	return redirectTo(request, getLanguageUrl(PAGE_URLS.ONBOARDING, language));
}

/** Requests the proxy runs for everything except the API, Next.js internals, auth callback, health check and files. */
export const config = {
	matcher: ["/((?!api|_next|callback|health|.*\\..*).*)"],
};
