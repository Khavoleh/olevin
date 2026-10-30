import { describe, expect, it } from "vitest";
import { getEnvelopeUrl } from "./get-envelope-url";

const DSN = "https://4f1c2a8e9b7d4e6f@ingest.internal.khavol.com/2";

const createEnvelope = (header: unknown) =>
	`${JSON.stringify(header)}\n{"type":"event"}\n{"message":"Error"}`;

describe("getEnvelopeUrl", () => {
	it("should return the envelope URL of the project", () => {
		expect(getEnvelopeUrl(createEnvelope({ dsn: DSN }), DSN)).toBe(
			"https://ingest.internal.khavol.com/api/2/envelope/",
		);
	});

	it("should keep the path prefix of the DSN", () => {
		const dsn = "https://4f1c2a8e9b7d4e6f@example.com/glitchtip/7";

		expect(getEnvelopeUrl(createEnvelope({ dsn }), dsn)).toBe(
			"https://example.com/glitchtip/api/7/envelope/",
		);
	});

	it.each([
		[
			"another project",
			"https://4f1c2a8e9b7d4e6f@ingest.internal.khavol.com/3",
		],
		["another key", "https://0000000000000000@ingest.internal.khavol.com/2"],
		["another host", "https://4f1c2a8e9b7d4e6f@attacker.example/2"],
	])("should reject a DSN of %s", (_, dsn) => {
		expect(getEnvelopeUrl(createEnvelope({ dsn }), DSN)).toBeUndefined();
	});

	it.each([
		["no DSN in the header", createEnvelope({ event_id: "1" })],
		["an invalid DSN", createEnvelope({ dsn: "not a url" })],
		["a header that is not JSON", "not json\n{}"],
		["an empty body", ""],
	])("should reject an envelope with %s", (_, envelope) => {
		expect(getEnvelopeUrl(envelope, DSN)).toBeUndefined();
	});

	it("should reject every envelope without a configured DSN", () => {
		expect(
			getEnvelopeUrl(createEnvelope({ dsn: DSN }), undefined),
		).toBeUndefined();
	});
});
