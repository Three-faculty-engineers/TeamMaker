import React from "react";
import { useLocation, Navigate, Outlet } from "react-router-dom";
import { useEffect, useState } from "react";

const RequireAuth = ({ allowedRole }) => {
    const location = useLocation();
    const [auth, setAuth] = useState(JSON.parse(sessionStorage.getItem("auth")))

    useEffect(() => {
        
    }, [])
    

    return (
        auth?.role >= allowedRole
            ? <Outlet />
            : auth?.user
                ? <Navigate to="/unauthorized" state={{ from: location }} replace />
                : <Navigate to="/login" state={{ from: location }} replace />
    );
}

export default RequireAuth;