import type { ReactNode } from "react";
import SettingsLabel from "./SettingsLabel";
import { SETTINGS_ROW_CLASSES } from "./settings-row-classes";

interface SettingsRowProps {
	label: string;
	hint?: string;
	hintTone?: "default" | "warning";
	children: ReactNode;
}

/** Setting with its own controls on the right. */
const SettingsRow = ({
	label,
	hint,
	hintTone,
	children,
}: Readonly<SettingsRowProps>) => (
	<div className={SETTINGS_ROW_CLASSES}>
		<SettingsLabel label={label} hint={hint} hintTone={hintTone} />
		<div className="flex flex-none items-center gap-1.5">{children}</div>
	</div>
);

export default SettingsRow;
