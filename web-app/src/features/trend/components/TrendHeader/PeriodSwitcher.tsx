"use client";

import { ToggleButton, ToggleButtonGroup } from "@shared/components";
import { useRouter } from "next/navigation";
import { startTransition, useOptimistic } from "react";

interface PeriodSwitcherProps {
	label: string;
	selected: string;
	options: { param: string; label: string }[];
}

/** Toggle of the trend period; the choice goes into the `period` URL parameter. */
const PeriodSwitcher = ({
	label,
	selected,
	options,
}: Readonly<PeriodSwitcherProps>) => {
	const router = useRouter();
	const [shown, setShown] = useOptimistic(selected);

	return (
		<ToggleButtonGroup
			aria-label={label}
			selectionMode="single"
			disallowEmptySelection
			selectedKeys={[shown]}
			onSelectionChange={(keys) => {
				const [param] = keys;

				if (typeof param !== "string" || param === shown) return;

				startTransition(() => {
					setShown(param);
					router.replace(`?period=${param}`, { scroll: false });
				});
			}}
		>
			{options.map((option) => (
				<ToggleButton key={option.param} id={option.param}>
					{option.label}
				</ToggleButton>
			))}
		</ToggleButtonGroup>
	);
};

export default PeriodSwitcher;
