import { ExportFormat } from "../../api/profile.schemas";

/** Formats of the snapshot export, as they are written in the URL. */
export const EXPORT_FORMATS = ["json", "csv"] as const;

const API_FORMATS = {
	json: ExportFormat.Json,
	csv: ExportFormat.Csv,
};

/** Gets the format of the API for a format of the URL, or `undefined` for an unknown one. */
export const getExportFormat = (
	format: string | null,
): ExportFormat | undefined =>
	EXPORT_FORMATS.find((known) => known === format)
		? API_FORMATS[format as (typeof EXPORT_FORMATS)[number]]
		: undefined;
