import { describe, expect, it } from "vitest";
import { ApiError } from "./api-error";

describe("ApiError", () => {
	it("should join the validation errors of a 400 response with commas", () => {
		const error = new ApiError({
			status: 400,
			data: {
				error: "Validation failed.",
				validationErrors: [
					"Timezone is unknown.",
					"Snapshot day is out of range.",
				],
			},
		});

		expect(error.message).toBe(
			"Timezone is unknown., Snapshot day is out of range.",
		);
	});

	it("should use the error of the response otherwise", () => {
		const error = new ApiError({
			status: 404,
			data: { error: "User not found.", validationErrors: null },
		});

		expect(error.message).toBe("User not found.");
		expect(error.status).toBe(404);
	});

	it("should fall back to the status when the response has no error", () => {
		expect(new ApiError({ status: 500 }).message).toBe(
			"The API responded with status 500. Contact the support, please.",
		);
	});
});
