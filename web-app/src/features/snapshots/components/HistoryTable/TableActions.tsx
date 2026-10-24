"use client";

import { PencilSimpleIcon, TrashIcon } from "@phosphor-icons/react";
import { Button, Link } from "@shared/components";
import { getI18n, getLanguageUrl } from "@shared/helpers";
import type { CalendarMonth } from "../../api/snapshots.schemas";
import { getSnapshotPath } from "../../helpers/get-snapshot-path/get-snapshot-path";
import { SNAPSHOTS_I18N } from "../../snapshots-i18n";

interface TableActionsProps {
	year: number;
	month: CalendarMonth;
	monthName: string;
	language: string;
	canDelete: boolean;
}

/** Icon buttons that change the month and, for an actual snapshot, delete it. */
const TableActions = ({
	year,
	month,
	monthName,
	language,
	canDelete,
}: Readonly<TableActionsProps>) => {
	const t = getI18n(language, SNAPSHOTS_I18N);

	return (
		<div className="flex justify-center gap-0.5">
			<Link
				variant="icon"
				href={getLanguageUrl(getSnapshotPath(year, month), language)}
				aria-label={`${t("edit")} ${monthName}`}
			>
				<PencilSimpleIcon size={18} />
			</Link>
			{canDelete ? (
				<Button
					variant="icon-danger"
					aria-label={`${t("delete")} ${monthName}`}
				>
					<TrashIcon size={18} />
				</Button>
			) : (
				<span aria-hidden className="size-11 flex-none" />
			)}
		</div>
	);
};

export default TableActions;
