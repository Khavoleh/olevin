import { handleSignIn } from "@logto/next/server-actions";
import { PAGE_URLS } from "@shared/constants";
import { getLogtoConfig } from "@shared/helpers";
import { redirect } from "next/navigation";
import type { NextRequest } from "next/server";

/**
 * The redirect URI registered in Logto: exchanges the authorization code for tokens,
 * saves them in the session cookie and returns to the app.
 */
export async function GET(request: NextRequest) {
	await handleSignIn(getLogtoConfig(), request.nextUrl.searchParams);

	redirect(PAGE_URLS.HOME);
}
