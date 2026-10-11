import { PAGE_URLS } from "@shared/constants";
import type { NavItemId } from "../../nav-item-id";

/**
 * Returns the navigation item that a page belongs to, or `undefined` for pages outside the navigation.
 * Takes the path without the language prefix. Editing a snapshot belongs to the history.
 */
export const getActiveNavItem = (page: string): NavItemId | undefined => {
	const path = page.replace(/\/+$/, "") || PAGE_URLS.HOME;

	if (path === PAGE_URLS.HOME) return "overview";
	if (path === PAGE_URLS.TREND) return "trend";
	if (path === PAGE_URLS.NEW_SNAPSHOT) return "new-snapshot";
	if (
		path === PAGE_URLS.SNAPSHOTS ||
		path.startsWith(`${PAGE_URLS.SNAPSHOTS}/`)
	)
		return "history";
	if (path === PAGE_URLS.PROFILE) return "profile";

	return undefined;
};
