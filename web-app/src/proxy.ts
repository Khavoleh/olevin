import {
	LANGUAGE_COOKIE,
	PAGE_URLS,
	PUBLIC_PAGE_URLS,
	TRACEPARENT_HEADER,
} from "@shared/constants";
import {
	createTraceparent,
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

/** Lets the request through with a new trace that every API call of its Server Action or page continues. */
function proceed(request: NextRequest): NextResponse {
	const headers = new Headers(request.headers);
	headers.set(TRACEPARENT_HEADER, createTraceparent());

	return NextResponse.next({ request: { headers } });
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
		return proceed(request);
	}

	if (!(await getIsAuthenticated(request))) {
		return redirectTo(request, getLanguageUrl(PAGE_URLS.SIGN_IN, language));
	}

	const isOnboarding =
		page === PAGE_URLS.ONBOARDING ||
		page.startsWith(`${PAGE_URLS.ONBOARDING}/`);

	if (isOnboarding || getIsOnboarded(request)) {
		return proceed(request);
	}

	return redirectTo(request, getLanguageUrl(PAGE_URLS.ONBOARDING, language));
}

/** Requests the proxy runs for everything except the API, Next.js internals, auth callback, health check and files. */
export const config = {
	matcher: ["/((?!api|_next|callback|health|.*\\..*).*)"],
};
