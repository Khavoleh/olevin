"use client";

import { PlusIcon } from "@phosphor-icons/react";
import { Link, Tooltip } from "@shared/components";
import { PAGE_URLS } from "@shared/constants";
import { formatDayMonth, getI18n, getLanguageUrl } from "@shared/helpers";
import { Focusable, TooltipTrigger } from "react-aria-components";
import { SNAPSHOTS_I18N } from "../../snapshots-i18n";
import { useSnapshotsStore } from "../../snapshots-store";

interface NewSnapshotButtonProps {
	language: string;
}

/** Link to a new snapshot; until a month opens it is disabled with a tooltip of when the next one opens. */
const NewSnapshotButton = ({ language }: Readonly<NewSnapshotButtonProps>) => {
	const t = getI18n(language, SNAPSHOTS_I18N);
	const history = useSnapshotsStore((state) => state.history);

	if (!history)
		return (
			<span aria-hidden className="block h-11 w-40 rounded-md bg-skeleton" />
		);

	const label = (
		<>
			<PlusIcon size={18} />
			{t("new_snapshot")}
		</>
	);

	if (history.newSnapshotOpensOn === null) {
		return (
			<Link
				variant="button"
				href={getLanguageUrl(PAGE_URLS.NEW_SNAPSHOT, language)}
			>
				{label}
			</Link>
		);
	}

	const hint = `${t("not_yet")} ${formatDayMonth(history.newSnapshotOpensOn, language)}.`;

	return (
		<TooltipTrigger delay={300}>
			<Focusable>
				<button
					type="button"
					aria-disabled="true"
					className="inline-flex min-h-11 cursor-not-allowed items-center justify-center gap-2 rounded-md bg-primary px-4.5 font-semibold text-on-primary opacity-45"
				>
					{label}
				</button>
			</Focusable>
			<Tooltip>{hint}</Tooltip>
		</TooltipTrigger>
	);
};

export default NewSnapshotButton;
