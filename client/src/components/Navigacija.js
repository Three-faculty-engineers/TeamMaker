
import React, { useState, useEffect } from "react";
import { Link, Navigate } from "react-router-dom";
import { Navbar, Container, Form, Nav, NavDropdown, Button, FormControl } from 'react-bootstrap'
import 'bootstrap/dist/css/bootstrap.min.css'
import Logo from '../img/863196ee0cdf449c854b29ffcbc7e563.png'

const Navigacija = (props) => {

  //nav.link > link

  const [auth, setAuth] = useState({})

  useEffect(() => {
    setAuth(JSON.parse(sessionStorage.getItem("auth")))
    
  }, [props.isLogged])
  
  
  return (
  <Navbar sticky="top" bg="light" expand="lg" style={{width:"100%"}}>
    <Container fluid>
      <Link to="/"><img className="Logo" src={Logo} alt="TEAMMAKER" /></Link>

      <Navbar.Toggle aria-controls="navbarScroll" />
      <Navbar.Collapse id="navbarScroll">
        <Nav
          className="me-auto my-2 my-lg-0"
          style={{ maxHeight: '100px' }}
          navbarScroll
        >
        <Nav.Link href="/about" className='obojenoHover' >ABOUT</Nav.Link>

        { (auth?.role >= 1 && auth?.user) && <Nav.Link onClick = {props.logout} to="/" className='obojenoHover'>LOGOUT</Nav.Link>}
        
        { (auth?.role >= 2 && auth?.user) &&  <Nav.Link href="/AdminPage" className='obojenoHover'>ADMIN PAGE</Nav.Link>}

        </Nav>
      </Navbar.Collapse>
    </Container>
  </Navbar>

  );
};
  
export default Navigacija;