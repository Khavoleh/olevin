import { getEnvelopeUrl } from "@shared/helpers";

/** Forwards the error reports of the browser to GlitchTip, which only the server can reach. */
export async function POST(request: Request) {
	const body = await request.arrayBuffer();
	const url = getEnvelopeUrl(
		new TextDecoder().decode(body),
		process.env.NEXT_PUBLIC_SENTRY_DSN,
	);

	if (!url) {
		return new Response(null, { status: 400 });
	}

	const response = await fetch(url, {
		method: "POST",
		headers: { "Content-Type": "application/x-sentry-envelope" },
		body,
	}).catch(() => undefined);

	return new Response(null, { status: response?.status ?? 502 });
}
