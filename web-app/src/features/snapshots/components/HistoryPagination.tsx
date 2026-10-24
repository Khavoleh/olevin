"use client";

import { CaretLeftIcon, CaretRightIcon } from "@phosphor-icons/react";
import { Button, Select, Text } from "@shared/components";
import { getI18n } from "@shared/helpers";
import type { GetSnapshotsResponse } from "../api/snapshots.schemas";
import { PAGE_SIZES } from "../constants/page-sizes";
import { SNAPSHOTS_I18N } from "../snapshots-i18n";
import { useSnapshotsStore } from "../snapshots-store";
import { useHistoryPage } from "../use-history-page";

interface HistoryPaginationProps {
	history: GetSnapshotsResponse;
	language: string;
}

const PAGE_SIZE_OPTIONS = PAGE_SIZES.map((size) => ({
	id: size,
	name: String(size),
}));

/** Rows per page and the arrows between pages; hidden while the whole history fits on the smallest page. */
const HistoryPagination = ({
	history,
	language,
}: Readonly<HistoryPaginationProps>) => {
	const t = getI18n(language, SNAPSHOTS_I18N);
	const page = useHistoryPage(history);
	const pageSize = useSnapshotsStore((state) => state.pageSize);
	const setPage = useSnapshotsStore((state) => state.setPage);
	const setPageSize = useSnapshotsStore((state) => state.setPageSize);

	if (page.total <= PAGE_SIZES[0]) return null;

	return (
		<nav
			aria-label={t("history")}
			className="flex flex-wrap items-center justify-between gap-3"
		>
			<Select
				label={t("page_size")}
				options={PAGE_SIZE_OPTIONS}
				value={pageSize}
				onChange={(size) => setPageSize(Number(size))}
			/>
			<div className="flex items-center gap-1">
				<Text variant="hint" elementType="span" className="px-2 tabular-nums">
					{`${page.from}–${page.to} ${t("range_of")} ${page.total}`}
				</Text>
				<Button
					variant="icon"
					aria-label={t("previous_page")}
					isDisabled={page.page === 1}
					onPress={() => setPage(page.page - 1)}
				>
					<CaretLeftIcon size={18} />
				</Button>
				<Button
					variant="icon"
					aria-label={t("next_page")}
					isDisabled={page.page === page.pageCount}
					onPress={() => setPage(page.page + 1)}
				>
					<CaretRightIcon size={18} />
				</Button>
			</div>
		</nav>
	);
};

export default HistoryPagination;
