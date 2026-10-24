interface SettingsLabelProps {
	label: string;
	hint?: string;
	hintId?: string;
	tone?: "default" | "danger";
	hintTone?: "default" | "warning";
}

/** Name of a setting with an optional hint under it. */
const SettingsLabel = ({
	label,
	hint,
	hintId,
	tone = "default",
	hintTone = "default",
}: Readonly<SettingsLabelProps>) => (
	<span className="grid min-w-0 gap-0.5">
		<span
			className={`font-semibold ${tone === "danger" ? "text-danger" : "text-fg"}`}
		>
			{label}
		</span>
		{hint && (
			<span
				id={hintId}
				aria-hidden={hintId ? true : undefined}
				className={`text-xs ${hintTone === "warning" ? "text-warning" : "text-fg-muted"}`}
			>
				{hint}
			</span>
		)}
	</span>
);

export default SettingsLabel;
