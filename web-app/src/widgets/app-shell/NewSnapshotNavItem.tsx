import { PlusIcon } from "@phosphor-icons/react/dist/ssr";
import { DisabledButton } from "@shared/components";
import { PAGE_URLS } from "@shared/constants";
import { formatDayMonth, getI18n, getLanguageUrl } from "@shared/helpers";
import { Suspense } from "react";
import AppNavLink from "./AppNavLink";
import { APP_SHELL_I18N } from "./app-shell-i18n";

interface NewSnapshotNavItemProps {
	language: string;
	/** Day the next month opens for a snapshot, or `null` while a month is open. */
	opensOn: Promise<string | null>;
	placement: "side" | "tab";
}

const PLACEMENT_CLASSES = {
	side: {
		item: "flex size-12 flex-none items-center justify-center gap-2 rounded-lg bg-primary font-semibold text-on-primary lg:h-11 lg:w-full lg:justify-start lg:rounded-md lg:px-3.5",
		open: "transition-colors hover:bg-primary-hover active:bg-primary-pressed",
		closed: "opacity-45",
	},
	tab: {
		item: "flex flex-col items-center gap-0.5 py-1 font-medium text-fg-muted text-xs",
		open: "aria-[current=page]:font-semibold aria-[current=page]:text-primary",
		closed: "opacity-45",
	},
};

/** Plus icon with the label that suits the place of the item. */
const ItemContent = ({
	language,
	placement,
}: Readonly<Omit<NewSnapshotNavItemProps, "opensOn">>) => {
	const t = getI18n(language, APP_SHELL_I18N);

	if (placement === "side") {
		return (
			<>
				<PlusIcon size={20} />
				<span className="sr-only lg:not-sr-only">{t("new_snapshot")}</span>
			</>
		);
	}

	return (
		<>
			<span className="flex h-7 w-12 items-center justify-center rounded-full bg-primary text-on-primary">
				<PlusIcon size={20} />
			</span>
			<span>{t("new_snapshot_short")}</span>
		</>
	);
};

/** Link to a new snapshot, or a disabled button with the day the next month opens. */
const NewSnapshotNavState = async ({
	language,
	opensOn,
	placement,
}: Readonly<NewSnapshotNavItemProps>) => {
	const t = getI18n(language, APP_SHELL_I18N);
	const date = await opensOn;
	const { item, open, closed } = PLACEMENT_CLASSES[placement];
	const label = placement === "tab" ? t("new_snapshot") : undefined;
	const content = <ItemContent language={language} placement={placement} />;

	if (date === null) {
		return (
			<AppNavLink
				id="new-snapshot"
				href={getLanguageUrl(PAGE_URLS.NEW_SNAPSHOT, language)}
				aria-label={label}
				className={`${item} ${open}`}
			>
				{content}
			</AppNavLink>
		);
	}

	return (
		<DisabledButton
			hint={t("not_yet").replace("{date}", formatDayMonth(date, language))}
			hasTooltip={placement === "side"}
			aria-label={label}
			className={`${item} ${closed}`}
		>
			{content}
		</DisabledButton>
	);
};

/** New snapshot item of the navigation, disabled until a month opens; on a phone the reason is left to screen readers. */
const NewSnapshotNavItem = (props: Readonly<NewSnapshotNavItemProps>) => {
	const { item, closed } = PLACEMENT_CLASSES[props.placement];

	return (
		<Suspense
			fallback={
				<span aria-hidden className={`${item} ${closed}`}>
					<ItemContent language={props.language} placement={props.placement} />
				</span>
			}
		>
			<NewSnapshotNavState {...props} />
		</Suspense>
	);
};

export default NewSnapshotNavItem;
