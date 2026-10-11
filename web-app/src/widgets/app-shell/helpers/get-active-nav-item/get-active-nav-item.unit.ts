import { describe, expect, it } from "vitest";
import { getActiveNavItem } from "./get-active-nav-item";

describe("getActiveNavItem", () => {
	it("should find the overview by the root", () => {
		expect(getActiveNavItem("/")).toBe("overview");
		expect(getActiveNavItem("")).toBe("overview");
	});

	it("should find the other sections", () => {
		expect(getActiveNavItem("/trend")).toBe("trend");
		expect(getActiveNavItem("/snapshots")).toBe("history");
		expect(getActiveNavItem("/profile")).toBe("profile");
	});

	it("should separate a new snapshot from the history", () => {
		expect(getActiveNavItem("/snapshots/new")).toBe("new-snapshot");
	});

	it("should count editing a snapshot as the history", () => {
		expect(getActiveNavItem("/snapshots/2026-09")).toBe("history");
	});

	it("should ignore a trailing slash", () => {
		expect(getActiveNavItem("/trend/")).toBe("trend");
	});

	it("should return undefined for pages outside the navigation", () => {
		expect(getActiveNavItem("/privacy")).toBeUndefined();
		expect(getActiveNavItem("/onboarding/3")).toBeUndefined();
	});

	it("should not mistake a similar path for a section", () => {
		expect(getActiveNavItem("/snapshots-archive")).toBeUndefined();
	});
});
