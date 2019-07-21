﻿/*
Grammar for Gd Language
*/

parser grammar GdGrammar;

options
{ tokenVocab = GdTokens; }

// Wegen Equals Token
@header {
    #pragma warning disable 0108
}

guiDescription 
    :   usingDeclaration* 
        namespaceDeclaration 
        EOF
    ;


usingDeclaration 
    :   usingDeclarationBegin 
            qualifiedName 
        usingDeclarationEnd
    ;

usingDeclarationBegin
    : Using
    ;

usingDeclarationEnd
    : End Using
    ;

namespaceDeclaration 
    :   namespaceDeclarationBegin
            container+ 
        namespaceDeclarationEnd
    ;

namespaceDeclarationBegin
    :   Namespace qualifiedName
    ;


namespaceDeclarationEnd
    :   End Namespace
    ;


container 
    :   form
    |   dialog
    |   usercontrol
    ;


form 
    :   formBegin
            containerDeclaration
        formEnd       
    ;

formBegin
    :   Form Identifier
    ;

formEnd
    :   End Form Identifier?   
    ;


dialog 
    :   dialogBegin
            containerDeclaration
        dialogEnd
    ;

dialogBegin
    :   Dialog Identifier
    ;

dialogEnd
    :   End Dialog Identifier?
    ;

usercontrol 
    :   userControlBegin 
            containerDeclaration
        userControlEnd
    ;

userControlBegin 
    : User Control Identifier
    ;

userControlEnd 
    : End User Control Identifier? 
    ;

containerDeclaration 
    :   template? 
        layoutSection? 
        propertiesSection? 
        hotkeysSection? 
        controlsSection? 
        nonVisualControlsSection?
    ;


layoutSection 
    :   layoutSectionBegin 
            property* 
        layoutSectionEnd
    ;

layoutSectionBegin
    :   Layout Identifier
    ;

layoutSectionEnd
    :    End Layout
    ;

layoutInfoSection
    :   layoutInfoSectionBegin 
            property* 
        layoutInfoSectionEnd
    ;

layoutInfoSectionBegin
    : LayoutInfo
    ;

layoutInfoSectionEnd
    : End LayoutInfo
    ;

eventsSection
    :   eventsSectionBegin 
            eventDeclaration+ 
        eventsSectionEnd
    ;

eventsSectionBegin
    : Events
    ;

eventsSectionEnd
    : End Events
    ;

eventDeclaration
    : EventName=Identifier PlusEquals CallName=Identifier CallType=Identifier?
    ;


propertiesSection
    :   propertiesSectionBegin 
            property* 
        propertiesSectionEnd
    ;

propertiesSectionBegin
    :  Properties
    ;

propertiesSectionEnd
    :  End Properties
    ;

hotkeysSection 
    :   hotkeysSectionBegin 
            hotkeyDeclaration+
        hotkeysSectionEnd
    ;

hotkeysSectionBegin
    :   Hotkeys
    ;

hotkeysSectionEnd
    :   End Hotkeys
    ;
	   

hotkeyDeclaration
    :   Hotkey Identifier modifierOption* hotkeyAssignement CallName=Identifier CallType=Identifier?
    ;

hotkeyAssignement
    :   plusEqualsHotkeyAssignement
    |   minusEqualsHotkeyAssignement
    ;

plusEqualsHotkeyAssignement
    : PlusEquals
    ;

minusEqualsHotkeyAssignement
    : MinusEquals
    ;
       
controlsSection 
    :   controlsSectionBegin 
            guiElement* 
        controlsSectionEnd
    ;

controlsSectionBegin
    :   Controls
    ;

controlsSectionEnd
    :   End Controls
    ;

guiElement
    :   panelSection 
    |   detailsPanelSection
    |   controlSection 
    |   barManagerSection
    |   tabNavigationSection
    |   multiViewSection
    ;


panelSection
    :   panelSectionBegin 
            layoutSection? 
            layoutInfoSection?
            propertiesSection?
            controlsSection?
        panelSectionEnd
    ;


panelSectionBegin
    : Panel Identifier template? 
    ;

panelSectionEnd
    : End Panel Identifier?
    ;

detailsPanelSection
    :   detailsPanelSectionBegin
            layoutSection? 
            layoutInfoSection? 
            propertiesSection? 
            controlsSection? 
        detailsPanelSectionEnd
    ;

detailsPanelSectionBegin
    :   DetailsPanel Identifier template?
    ;

detailsPanelSectionEnd
    :   End DetailsPanel Identifier?
    ;

controlSection
    :   controlSectionBegin 
            layoutInfoSection? 
            propertiesSection? 
            eventsSection? 
            hotkeysSection?
            contextMenuSection? 
            bindingSection? 
        controlSectionEnd
    ;     
            

controlSectionBegin
    : Control ControlType=Identifier Identifier template?
    ;

controlSectionEnd
    : End Control Identifier?
    ;

barManagerSection
    :   barManagerSectionBegin 
            propertiesSection? 
            controlsSection? 
        barManagerSectionEnd
    ;
      

barManagerSectionBegin
    :   BarManager Identifier
    ;

barManagerSectionEnd
    :   End BarManager Identifier?
    ;

tabNavigationSection
    :   tabNavigationSectionBegin 
            propertiesSection? 
            eventsSection?
            hotkeysSection? 
            contextMenuSection?  
            tabsSection? 
            sharedControlSection? 
        tabNavigationSectionEnd
      ;

tabNavigationSectionBegin
    :  TabNavigation Identifier
    ;

tabNavigationSectionEnd
    :  End TabNavigation Identifier?
    ;

tabsSection
    :   tabsSectionBegin 
            tabPageSection* 
        tabsSectionEnd
      ;
      
tabsSectionBegin
    :   Tabs
    ;

tabsSectionEnd
    :   End Tabs
    ;

tabPageSection
    :   tabPageSectionBegin
            propertiesSection? 
            eventsSection? 
            contextMenuSection? 
            controlsSection? 
        tabPageSectionEnd
    ;

tabPageSectionBegin
    :   TabPage Identifier template? 
    ;

tabPageSectionEnd
    :   End TabPage Identifier?
    ;


multiViewSection
    :   multiViewSectionBegin 
            layoutInfoSection? 
            propertiesSection? 
            eventsSection? 
            hotkeysSection? 
            contextMenuSection? 
            userControlsSection?
        multiViewSectionEnd
    ;


multiViewSectionBegin
    :  MultiView Identifier template?;


multiViewSectionEnd
    :  End MultiView Identifier?;


sharedControlSection
    :   sharedControlSectionBegin
            controlSection* 
        sharedControlSectionEnd
    ;

sharedControlSectionBegin
    : SharedControl
    ;

sharedControlSectionEnd
    : End SharedControl
    ;


nonVisualControlsSection
    :   nonVisualControlsSectionBegin 
            controlSection* 
        nonVisualControlsSectionEnd
    ;

nonVisualControlsSectionBegin
    :   NonVisualControls
    ;

nonVisualControlsSectionEnd
    :   End NonVisualControls
    ;

userControlsSection 
    :   userControlsSectionBegin 
            controlSection* 
        userControlsSectionEnd
    ;

userControlsSectionBegin
    :   Controls
    ;

userControlsSectionEnd
    :   End Controls
    ;

contextMenuSection
    :   contextMenuSectionBegin 
            property* 
        contextMenuSectionEnd
    ;

contextMenuSectionBegin
    : ContextMenu
    ;

contextMenuSectionEnd
    : End ContextMenu
    ;

bindingSection
    :   bindingSectionBegin
        bindingSectionEnd
    ;

bindingSectionBegin
    : Binding
    ;

bindingSectionEnd
    : End Binding
    ;

modifierOption
    : Comma modifier
    ;

modifier
    : plusCtrlModifier 
    | minusCtrlModifier 
    | plusAltModifier 
    | minusAltModifier 
    | plusShiftModifier 
    | minusShiftModifier
    ;

plusCtrlModifier
    : PlusCtrl
    ;

minusCtrlModifier
    : MinusCtrl
    ;

plusAltModifier
    : PlusAlt
    ;

minusAltModifier
    : MinusAlt
    ;

plusShiftModifier
    : PlusShift
    ;

minusShiftModifier
    : MinusShift
    ;
property 
    : propertyAssign
    | propertyAddAssign
    ;
    
    propertyAssign
    : lvalueExpression Equals rvalue
    ;

    // TODO rvalues in rules und damit in Syntaxen umwandeln
rvalue
    : identifierValue
    | integerValue
    | stringValue
    | characterValue
    | trueValue
    | falseValue           
    ;

identifierValue
    : Identifier
    ;

integerValue
    : Integer
    ;

stringValue
    : String
    ;

characterValue
    : Character
    ;

trueValue
    : True
    ;

falseValue
    : False
    ;


propertyAddAssign
    :   lvalueExpression PlusEquals Identifier CallType=Identifier?
    ;


lvalueExpression 
    : memberAccessExpression lvalueExpressionContinuation?
    ;

lvalueExpressionContinuation
    : Dot lvalueExpression
    ;

memberAccessExpression
    : simpleMemberAccessExpression 
    | elementAccessExpression
    ;


simpleMemberAccessExpression
    : identifierName
    ;


elementAccessExpression
    : identifierName OpenBracket Integer CloseBracket
    ;
    

template 
    : Template Equals qualifiedName
    ;


identifierName
    : Identifier
    ;


qualifiedName 
    : identifierName qualifiedNameContinuation?
    ;

qualifiedNameContinuation
    : Dot qualifiedName
    ;