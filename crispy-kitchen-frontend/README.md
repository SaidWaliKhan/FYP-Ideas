# Crispy Kitchen frontend

This React and Vite application provides the customer, kitchen, and admin interfaces for Crispy Kitchen.

## Run locally

1. Start the API at `http://localhost:5001`.
2. Set `VITE_API_URL=http://localhost:5001/api` in `.env`.
3. Run `npm install`, then `npm run dev`.

## Screens

- `/menu` — public menu and customer checkout.
- `/login` and `/register` — customer authentication.
- `/orders/mine` — customer order history.
- `/kitchen` — kitchen workflow for Admin and KitchenStaff.
- `/admin` — product inventory and staff management for Admin.

## Scripts

- `npm run dev` starts the development server.
- `npm run build` creates a production build.
- `npm run lint` checks source code with ESLint.
