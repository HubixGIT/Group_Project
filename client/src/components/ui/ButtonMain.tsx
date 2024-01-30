import classNames from 'classnames';

interface Props {
  className?: string;
  disabled?: boolean;
  text: string;
  type: 'submit' | 'reset' | 'button' | undefined;
}

export default function ButtonMain({ className, disabled, text, type }: Props) {
  return (
    <button
      className={classNames(
        'h-10 rounded-md bg-[#0065ff] text-sm font-semibold text-white',
        className,
      )}
      type={type}
      disabled={disabled}
    >
      {text}
    </button>
  );
}
