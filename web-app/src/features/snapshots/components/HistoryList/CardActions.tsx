"use client";

import { PencilSimpleIcon, TrashIcon } from "@phosphor-icons/react";
import { Button, Link } from "@shared/components";
import { getI18n, getLanguageUrl } from "@shared/helpers";
import type { CalendarMonth } from "../../api/snapshots.schemas";
import { getSnapshotPath } from "../../helpers/get-snapshot-path/get-snapshot-path";
import { SNAPSHOTS_I18N } from "../../snapshots-i18n";

interface CardActionsProps {
	year: number;
	month: CalendarMonth;
	monthName: string;
	language: string;
	canDelete: boolean;
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
	canDelete,
}: Readonly<CardActionsProps>) => {
	const t = getI18n(language, SNAPSHOTS_I18N);

	return (
		<div
			className={`grid border-t ${canDelete ? ROW_CLASSES.withDelete : ROW_CLASSES.editOnly}`}
		>
			<Link
				variant="action"
				href={getLanguageUrl(getSnapshotPath(year, month), language)}
				aria-label={`${t("edit")} ${monthName}`}
				className={`${ACTION_CLASSES} ${canDelete ? EDIT_CORNER_CLASSES.withDelete : EDIT_CORNER_CLASSES.editOnly}`}
			>
				<PencilSimpleIcon size={17} />
				{t("edit")}
			</Link>
			{canDelete && <span aria-hidden className="bg-border" />}
			{canDelete && (
				<Button
					variant="danger"
					aria-label={`${t("delete")} ${monthName}`}
					className={`${ACTION_CLASSES} rounded-br-[calc(var(--radius-lg)-1px)]`}
				>
					<TrashIcon size={17} />
					{t("delete")}
				</Button>
			)}
		</div>
	);
};

export default CardActions;
