"use client";

import { Label, Text } from "@shared/components";
import { type ReactNode, useId } from "react";
import { BLOCK_CLASSES } from "../constants/block-classes";

interface FormBlockProps {
	title: string;
	hint?: string;
	children: ReactNode;
}

/** Bordered block of related fields with its title and hint. */
const FormBlock = ({ title, hint, children }: Readonly<FormBlockProps>) => {
	const titleId = useId();

	return (
		<fieldset aria-labelledby={titleId} className={`min-w-0 ${BLOCK_CLASSES}`}>
			<Label variant="group" elementType="span" id={titleId}>
				{title}
			</Label>
			{hint && <Text variant="hint">{hint}</Text>}
			{children}
		</fieldset>
	);
};

export default FormBlock;
