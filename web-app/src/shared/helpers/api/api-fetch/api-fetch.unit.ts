import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { z } from "zod";
import { ApiError } from "../api-error";
import { apiFetch } from "./api-fetch";

const schema = z.object({ subject: z.string() });

describe("apiFetch", () => {
	const fetchMock = vi.fn();

	beforeEach(() => {
		vi.stubEnv("API_URL", "http://api.test/");
		vi.stubGlobal("fetch", fetchMock);
	});

	afterEach(() => {
		vi.unstubAllEnvs();
		vi.unstubAllGlobals();
		fetchMock.mockReset();
	});

	it("should call the API with the token and return the validated body", async () => {
		fetchMock.mockResolvedValue(Response.json({ subject: "user-1" }));

		await expect(apiFetch("/me", schema, "token")).resolves.toEqual({
			subject: "user-1",
		});
		expect(fetchMock).toHaveBeenCalledWith("http://api.test/me", {
			headers: { Accept: "application/json", Authorization: "Bearer token" },
		});
	});

	it("should throw ApiError with the status when the API fails", async () => {
		fetchMock.mockResolvedValue(new Response(null, { status: 401 }));

		await expect(apiFetch("/me", schema, "token")).rejects.toMatchObject({
			name: "ApiError",
			status: 401,
		});
		await expect(apiFetch("/me", schema, "token")).rejects.toBeInstanceOf(
			ApiError,
		);
	});

	it("should reject a body that does not match the schema", async () => {
		fetchMock.mockResolvedValue(Response.json({ subject: 1 }));

		await expect(apiFetch("/me", schema, "token")).rejects.toThrow();
	});
});
