// Expects an image at public/cover.webp (served at /cover.webp). Swap the
// file directly to try different images - no code change needed as long
// as the filename/path stays the same.
export function CoverBanner() {
  return (
    <div className="cover-banner">
      <img
        src="/cover.webp"
        alt="The Colombo Fort to Badulla scenic highlands railway line"
      />
      <div className="cover-banner-scrim" aria-hidden="true" />
      <div className="cover-banner-text">
        <h1>Colombo Fort &ndash; Badulla</h1>
        <p>Segment-based reserved seat booking</p>
      </div>
    </div>
  );
}
