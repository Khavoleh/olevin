import { PAGE_URLS } from "./page-urls";

/** Pages that open without signing in. Every other page requires a session. */
export const PUBLIC_PAGE_URLS = [
	PAGE_URLS.SIGN_IN,
	PAGE_URLS.PRIVACY,
	PAGE_URLS.TERMS,
];
