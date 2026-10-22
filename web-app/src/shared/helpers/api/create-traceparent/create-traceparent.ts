const getRandomHex = (bytes: number): string =>
	Array.from(crypto.getRandomValues(new Uint8Array(bytes)), (byte) =>
		byte.toString(16).padStart(2, "0"),
	).join("");

/** New W3C `traceparent` value with random trace and span ids. */
export const createTraceparent = (): string =>
	`00-${getRandomHex(16)}-${getRandomHex(8)}-01`;
