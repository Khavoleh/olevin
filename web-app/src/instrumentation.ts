import * as Sentry from "@sentry/nextjs";

/** Reports the server errors of Next.js to GlitchTip and links them with the errors of the API calls. */
export function register() {
	Sentry.init({
		dsn: process.env.NEXT_PUBLIC_SENTRY_DSN,
		environment: process.env.NODE_ENV,
		tracePropagationTargets: process.env.API_URL ? [process.env.API_URL] : [],
	});
}

/** Reports errors of Server Components, Server Actions, route handlers and the proxy. */
export const onRequestError = Sentry.captureRequestError;
