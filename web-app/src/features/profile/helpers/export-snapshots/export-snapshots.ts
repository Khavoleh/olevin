import { requestOrRedirect } from "@shared/api";
import { ApiError } from "@shared/helpers";
import { exportSnapshots as requestExport } from "../../api/profile";
import { getExportFormat } from "../get-export-format/get-export-format";

/** Gives every snapshot of the user as a file for a download; a failure is a JSON response with its status and error. */
export async function exportSnapshots(
	format: string | null,
	language: string,
): Promise<Response> {
	const apiFormat = getExportFormat(format);

	if (!apiFormat) {
		return Response.json({ error: "Unknown export format." }, { status: 400 });
	}

	const response = await requestOrRedirect(
		() => requestExport({ format: apiFormat }),
		language,
	);

	if (response.status !== 200) {
		return Response.json(
			{ error: new ApiError(response).message },
			{ status: response.status },
		);
	}

	return new Response(response.data as Blob, {
		headers: {
			"Content-Type": response.headers.get("content-type") ?? "",
			"Content-Disposition": response.headers.get("content-disposition") ?? "",
			"Cache-Control": "no-store",
		},
	});
}
