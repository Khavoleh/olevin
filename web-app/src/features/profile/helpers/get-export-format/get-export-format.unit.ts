import { describe, expect, it } from "vitest";
import { ExportFormat } from "../../api/profile.schemas";
import { getExportFormat } from "./get-export-format";

describe("getExportFormat", () => {
	it("should give the format of the API for a format of the URL", () => {
		expect(getExportFormat("json")).toBe(ExportFormat.Json);
		expect(getExportFormat("csv")).toBe(ExportFormat.Csv);
	});

	it("should reject an unknown or missing format", () => {
		for (const format of ["xml", "JSON", "", null]) {
			expect(getExportFormat(format)).toBeUndefined();
		}
	});

	it("should reject the keys of Object.prototype", () => {
		for (const format of ["constructor", "toString", "__proto__"]) {
			expect(getExportFormat(format)).toBeUndefined();
		}
	});
});
