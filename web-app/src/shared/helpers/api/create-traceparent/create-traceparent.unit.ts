import { describe, expect, it } from "vitest";
import { createTraceparent } from "./create-traceparent";

describe("createTraceparent", () => {
	it("should follow the W3C format with a sampled flag", () => {
		expect(createTraceparent()).toMatch(/^00-[0-9a-f]{32}-[0-9a-f]{16}-01$/);
	});

	it("should create a different trace on every call", () => {
		expect(createTraceparent()).not.toBe(createTraceparent());
	});
});
