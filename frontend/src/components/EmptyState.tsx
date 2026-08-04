// Small hand-drawn icons (no icon library dependency) for the two families
// of "nothing to show yet" messages in the booking flow: date/departure
// related, and origin/destination related.
const icons = {
  calendar: (
    <svg
      width="22"
      height="22"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="1.5"
      strokeLinecap="round"
      strokeLinejoin="round"
      aria-hidden="true"
    >
      <rect x="3" y="5" width="18" height="16" rx="2" />
      <line x1="3" y1="10" x2="21" y2="10" />
      <line x1="8" y1="3" x2="8" y2="7" />
      <line x1="16" y1="3" x2="16" y2="7" />
      <line x1="5" y1="19" x2="19" y2="7" />
    </svg>
  ),
  route: (
    <svg
      width="22"
      height="22"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="1.5"
      strokeLinecap="round"
      strokeLinejoin="round"
      aria-hidden="true"
    >
      <circle cx="6" cy="6" r="2.5" />
      <circle cx="18" cy="18" r="2.5" />
      <path d="M6 8.5 C6 14, 18 10, 18 15.5" strokeDasharray="3 3" />
    </svg>
  ),
};

interface EmptyStateProps {
  icon: keyof typeof icons;
  message: string;
}

export function EmptyState({ icon, message }: EmptyStateProps) {
  return (
    <div className="empty-state">
      {icons[icon]}
      <span>{message}</span>
    </div>
  );
}
