interface LegalListProps {
	items: string[];
}

/** Bulleted list of a legal document section. */
const LegalList = ({ items }: Readonly<LegalListProps>) => {
	return (
		<ul className="flex list-disc flex-col gap-1.5 pl-5 text-base text-fg">
			{items.map((item) => (
				<li key={item}>{item}</li>
			))}
		</ul>
	);
};

export default LegalList;
