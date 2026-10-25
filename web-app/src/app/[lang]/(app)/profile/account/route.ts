import { getAccountCenterUrl } from "@features/profile";
import { getLogtoConfig } from "@shared/helpers";
import { redirect } from "next/navigation";
import type { NextRequest } from "next/server";

interface AccountCenterRouteContext {
	params: Promise<{ lang: string }>;
}

/** Opens the security page of the Logto Account Center in the language of the app. */
export async function GET(
	_request: NextRequest,
	{ params }: AccountCenterRouteContext,
) {
	const { lang } = await params;

	redirect(getAccountCenterUrl(getLogtoConfig().endpoint, lang));
}
