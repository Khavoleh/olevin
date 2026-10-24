"use client";

import { PencilSimpleIcon, TrashIcon } from "@phosphor-icons/react";
import { Button, Link } from "@shared/components";
import { getI18n, getLanguageUrl } from "@shared/helpers";
import { useState } from "react";
import type { CalendarMonth } from "../../api/history.schemas";
import { getSnapshotPath } from "../../helpers/get-snapshot-path/get-snapshot-path";
import { HISTORY_I18N } from "../../history-i18n";
import type { DeletableSnapshot } from "../../interfaces/deletable-snapshot";
import DeleteSnapshotDialog from "../DeleteSnapshotDialog";

interface TableActionsProps {
	year: number;
	month: CalendarMonth;
	monthName: string;
	language: string;
	/** Actual snapshot the row can delete; an estimate can only be changed. */
	deletable?: DeletableSnapshot;
}

/** Icon buttons that change the month and, for an actual snapshot, delete it. */
const TableActions = ({
	year,
	month,
	monthName,
	language,
	deletable,
}: Readonly<TableActionsProps>) => {
	const t = getI18n(language, HISTORY_I18N);
	const [isDeleting, setIsDeleting] = useState(false);

	return (
		<div className="flex justify-center gap-0.5">
			<Link
				variant="icon"
				href={getLanguageUrl(getSnapshotPath(year, month), language)}
				aria-label={`${t("edit")} ${monthName}`}
			>
				<PencilSimpleIcon size={18} />
			</Link>
			{deletable ? (
				<>
					<Button
						variant="icon-danger"
						aria-label={`${t("delete")} ${monthName}`}
						isDisabled={deletable.isOnly}
						onPress={() => setIsDeleting(true)}
					>
						<TrashIcon size={18} />
					</Button>
					<DeleteSnapshotDialog
						snapshot={deletable}
						monthName={monthName}
						language={language}
						isOpen={isDeleting}
						onOpenChange={setIsDeleting}
					/>
				</>
			) : (
				<span aria-hidden className="size-11 flex-none" />
			)}
		</div>
	);
};

export default TableActions;
