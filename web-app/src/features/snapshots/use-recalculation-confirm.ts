import type { SubmissionResult } from "@conform-to/react";
import { type SyntheticEvent, useEffect, useRef, useState } from "react";

/** Holds back a valid form until the user confirms that the later snapshots are recalculated. */
export const useRecalculationConfirm = (
	laterSnapshots: number,
	lastResult: SubmissionResult | undefined,
) => {
	const formRef = useRef<HTMLFormElement>(null);
	const isConfirmed = useRef(false);
	const [isOpen, setIsOpen] = useState(false);

	useEffect(() => {
		if (!lastResult) return;

		isConfirmed.current = false;
		setIsOpen(false);
	}, [lastResult]);

	return {
		formRef,
		isOpen,
		setIsOpen,
		onSubmit: (event: SyntheticEvent) => {
			if (laterSnapshots > 0 && !isConfirmed.current) {
				event.preventDefault();
				setIsOpen(true);
			}
		},
		confirm: () => {
			isConfirmed.current = true;
			formRef.current?.requestSubmit();
		},
	};
};
