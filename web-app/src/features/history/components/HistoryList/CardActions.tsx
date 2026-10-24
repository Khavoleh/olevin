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

interface CardActionsProps {
	year: number;
	month: CalendarMonth;
	monthName: string;
	language: string;
	/** Actual snapshot the card can delete; an estimate can only be changed. */
	deletable?: DeletableSnapshot;
}

const ROW_CLASSES = {
	withDelete: "grid-cols-[1fr_1px_1fr] border-border",
	editOnly: "grid-cols-1 border-input border-dashed",
};

const EDIT_CORNER_CLASSES = {
	withDelete: "rounded-bl-[calc(var(--radius-lg)-1px)]",
	editOnly: "rounded-b-[calc(var(--radius-lg)-1px)]",
};

/** Action that fills a bottom corner of the card, with its focus ring drawn inside it. */
const ACTION_CLASSES = "text-sm -outline-offset-2";

/** Row of labelled buttons under a card that change the month and, for an actual snapshot, delete it. */
const CardActions = ({
	year,
	month,
	monthName,
	language,
	deletable,
}: Readonly<CardActionsProps>) => {
	const t = getI18n(language, HISTORY_I18N);
	const [isDeleting, setIsDeleting] = useState(false);
	const layout = deletable ? "withDelete" : "editOnly";

	return (
		<div className={`grid border-t ${ROW_CLASSES[layout]}`}>
			<Link
				variant="action"
				href={getLanguageUrl(getSnapshotPath(year, month), language)}
				aria-label={`${t("edit")} ${monthName}`}
				className={`${ACTION_CLASSES} ${EDIT_CORNER_CLASSES[layout]}`}
			>
				<PencilSimpleIcon size={17} />
				{t("edit")}
			</Link>
			{deletable && <span aria-hidden className="bg-border" />}
			{deletable && (
				<Button
					variant="danger"
					aria-label={`${t("delete")} ${monthName}`}
					isDisabled={deletable.isOnly}
					onPress={() => setIsDeleting(true)}
					className={`${ACTION_CLASSES} rounded-br-[calc(var(--radius-lg)-1px)]`}
				>
					<TrashIcon size={17} />
					{t("delete")}
				</Button>
			)}
			{deletable && (
				<DeleteSnapshotDialog
					snapshot={deletable}
					monthName={monthName}
					language={language}
					isOpen={isDeleting}
					onOpenChange={setIsDeleting}
				/>
			)}
		</div>
	);
};

export default CardActions;
