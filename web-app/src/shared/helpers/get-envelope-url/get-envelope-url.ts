import { z } from "zod";

/** First line of a Sentry envelope with the DSN the browser SDK sends to. */
const envelopeHeaderSchema = z.object({
	dsn: z.url(),
});

/** Reads the header line of an envelope. */
const parseHeader = (envelope: string): unknown => {
	try {
		return JSON.parse(envelope.split("\n", 1)[0] ?? "");
	} catch {
		return undefined;
	}
};

/** Gets the GlitchTip envelope URL of the DSN. Returns `undefined` when the envelope is sent to another DSN. */
export const getEnvelopeUrl = (
	envelope: string,
	dsn: string | undefined,
): string | undefined => {
	const header = envelopeHeaderSchema.safeParse(parseHeader(envelope));
	const expected = z.url().safeParse(dsn);

	if (!header.success || !expected.success) {
		return undefined;
	}

	const url = new URL(expected.data);

	if (new URL(header.data.dsn).href !== url.href) {
		return undefined;
	}

	const separator = url.pathname.lastIndexOf("/");
	const path = url.pathname.slice(0, separator);
	const projectId = url.pathname.slice(separator + 1);

	return `${url.origin}${path}/api/${projectId}/envelope/`;
};
