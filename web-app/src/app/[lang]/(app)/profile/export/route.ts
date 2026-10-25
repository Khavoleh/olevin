import { exportSnapshots } from "@features/profile";
import type { NextRequest } from "next/server";

interface ExportRouteContext {
	params: Promise<{ lang: string }>;
}

/** Downloads every snapshot of the user as a JSON or CSV file, by the `format` of the query. */
export async function GET(
	request: NextRequest,
	{ params }: ExportRouteContext,
) {
	const { lang } = await params;

	return exportSnapshots(request.nextUrl.searchParams.get("format"), lang);
}
