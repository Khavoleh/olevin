"use client";

import { type ReactNode, useId } from "react";
import Label from "./Label";
import Text from "./Text";

interface FormBlockProps {
	title?: string;
	hint?: string;
	children: ReactNode;
}

const BLOCK_CLASSES =
	"grid min-w-0 gap-2.5 rounded-lg border border-border bg-surface p-4";

/** Bordered block of a form; with a title it groups its fields under it. */
const FormBlock = ({ title, hint, children }: Readonly<FormBlockProps>) => {
	const titleId = useId();

	if (!title) return <div className={BLOCK_CLASSES}>{children}</div>;

	return (
		<fieldset aria-labelledby={titleId} className={BLOCK_CLASSES}>
			<Label variant="group" elementType="span" id={titleId}>
				{title}
			</Label>
			{hint && <Text variant="hint">{hint}</Text>}
			{children}
		</fieldset>
	);
};

export default FormBlock;
